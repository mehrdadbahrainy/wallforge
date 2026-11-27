"""Disposable WallForge container only. Never prints credentials or cookies."""
import hashlib, http.cookiejar, json, os, secrets, ssl, subprocess, time, urllib.request, urllib.error

results = []
def check(test_id, scenario, condition, actual):
    result = dict(id=test_id, scenario=scenario, status='PASS' if condition else 'FAIL', actual=actual)
    results.append(result)
    print(json.dumps(result), flush=True)

def cmd(args, **kwargs):
    return subprocess.run(args, capture_output=True, text=True, **kwargs)

def ufw_hash():
    h = hashlib.sha256()
    for root, dirs, files in os.walk('/etc/ufw'):
        for name in sorted(files):
            path = os.path.join(root, name)
            h.update(path.encode()); h.update(open(path, 'rb').read())
    return h.hexdigest()

ctx = ssl.create_default_context(cafile='/opt/wallforge/tls/localhost.crt')
base = os.environ.get('WF_BASE', 'https://localhost:5443')

# A detached container launch returns before Kestrel is listening. Wait for verified HTTPS.
for attempt in range(40):
    try:
        with urllib.request.urlopen(base+'/api/health', context=ctx, timeout=2) as health:
            if health.status==200: break
    except (urllib.error.URLError, TimeoutError): pass
    time.sleep(.25)
else: raise RuntimeError('WallForge HTTPS health did not become ready')

def client():
    jar = http.cookiejar.CookieJar()
    return urllib.request.build_opener(urllib.request.HTTPSHandler(context=ctx), urllib.request.HTTPCookieProcessor(jar)), jar
def request(c, path, data=None, token=None, method=None, origin=None):
    headers = {}
    if data is not None: headers['Content-Type'] = 'application/json'
    if token: headers['X-WallForge-CSRF'] = token
    if origin: headers['Origin'] = origin
    req = urllib.request.Request(base + path, data=json.dumps(data).encode() if data is not None else None, headers=headers, method=method)
    try:
        with c.open(req, timeout=15) as r: return r.status, r.read().decode(), r.headers
    except urllib.error.HTTPError as e: return e.code, e.read().decode(), e.headers
def csrf(c):
    status, body, _ = request(c, '/api/auth/csrf')
    assert status == 200, status
    return json.loads(body)['token']

password = secrets.token_hex(24)
users = ['wf-val-admin', 'wf-val-viewer', 'wf-val-outsider']
created = []
before = ufw_hash()
try:
    for user, group in zip(users, ['wallforge-admin', 'wallforge-viewer', None]):
        assert cmd(['id', user]).returncode != 0, 'Fixture already exists; refusing to alter it'
        args = ['useradd', '-M', '-s', '/bin/bash']
        if group: args += ['-G', group]
        assert cmd(args + [user]).returncode == 0
        created.append(user)
        assert cmd(['chpasswd'], input=f'{user}:{password}\n').returncode == 0
    harness = '/tmp/wallforge-validation-integration/WallForge.LinuxIntegration'
    for tid, user, role, mode, pwd in [
        ('LIN-01',users[0],'admin','read-ufw',password),
        ('LIN-02',users[1],'viewer','valid',password),
        ('LIN-03',users[0],'admin','invalid-password','wrong-validation-password'),
        ('LIN-04',users[2],'none','valid',password)]:
        r = cmd(['runuser','-u','wallforge-api','--',harness,user,role,mode], input=pwd+'\n')
        check(tid, f'Linux PAM/group harness: {role}/{mode}', r.returncode == 0, r.stdout.strip() or 'Harness failed')
    cmd(['usermod','--expiredate','1',users[0]])
    r=cmd(['runuser','-u','wallforge-api','--',harness,users[0],'admin','account-disabled'],input=password+'\n')
    check('LIN-05','Expired PAM account denied',r.returncode==0,r.stdout.strip() or 'Harness failed')
    cmd(['usermod','--expiredate','-1',users[0]])
    r=cmd(['runuser','-u','wallforge-api','--','sudo','-n','/usr/bin/id'])
    check('SEC-01','Arbitrary sudo command denied',r.returncode!=0,'exit='+str(r.returncode))
    anon,_=client()
    status,body,_=request(anon,'/api/health')
    check('API-01','HTTPS health with certificate validation',status==200 and json.loads(body)['status']=='ok',f'{status}: {body}')
    for path in ['/api/auth/me','/api/host/users','/api/host/firewall','/api/audit']:
        status,_,_=request(anon,path)
        check('API-02-'+path.split('/')[-1],'Unauthenticated '+path,status==401,str(status))
    status,_,_=request(anon,'/api/auth/login',dict(userName=users[0],password='wrong'))
    check('SEC-02','Login without CSRF rejected',status==400,str(status))
    token=csrf(anon)
    status,body,_=request(anon,'/api/auth/login',dict(userName=users[0],password='wrong'),token)
    check('AUTH-01','Invalid password rejected without secret details',status==401 and password not in body,str(status))
    status,_,_=request(anon,'/api/auth/login',dict(userName='root',password='x'),token)
    check('AUTH-02','Root input rejected',status==400,str(status))
    admin,jar=client(); old_token=csrf(admin)
    status,body,headers=request(admin,'/api/auth/login',dict(userName=users[0],password=password),old_token)
    check('AUTH-03','PAM admin login',status==200 and json.loads(body).get('role')=='admin',str(status))
    cookies=headers.get_all('Set-Cookie') or []
    check('SEC-03','Secure HttpOnly Strict session cookie',any('__Host-wallforge-session=' in c and 'secure' in c.lower() and 'httponly' in c.lower() and 'samesite=strict' in c.lower() for c in cookies),'Required flags present' if cookies else 'No cookie')
    for path in ['/api/host/users','/api/host/firewall','/api/audit']:
        status,body,headers=request(admin,path)
        check('API-03-'+path.split('/')[-1],'Admin read '+path,status==200,f'{status}; cache-control={headers.get("Cache-Control","absent")}')
        check('SEC-09-'+path.split('/')[-1],'Sensitive response forbids caching',headers.get('Cache-Control')=='no-store',headers.get('Cache-Control','absent'))
        if path.endswith('users'):
            snapshot=json.loads(body)
            check('API-04','Inventory includes fixture users/groups',all(u in [x['userName'] for x in snapshot['users']] for u in users) and any(g['name']=='wallforge-admin' for g in snapshot['groups']),'Fixture identities present')
        if path.endswith('firewall'):
            check('UFW-01','UFW inactive status and no rules',json.loads(body).get('status')=='inactive' and json.loads(body).get('rules')==[],body)
    status,_,_=request(admin,'/api/host/firewall',{},method='POST')
    check('SEC-04','Firewall mutation endpoint denied',status==405,str(status))
    status,_,_=request(admin,'/api/auth/logout',{},old_token)
    check('SEC-05','Anonymous token invalid after login',status==400,str(status))
    token=csrf(admin)
    status,_,_=request(admin,'/api/auth/logout',{},token,origin='https://evil.invalid')
    check('SEC-06','Cross-origin logout rejected',status==400,str(status))
    cmd(['gpasswd','-d',users[0],'wallforge-admin'])
    status,_,_=request(admin,'/api/auth/me')
    check('AUTH-04','Live group removal revokes session',status==401,str(status))
    cmd(['usermod','-aG','wallforge-admin',users[0]])
    viewer,vjar=client(); token=csrf(viewer)
    status,body,_=request(viewer,'/api/auth/login',dict(userName=users[1],password=password),token)
    check('AUTH-05','PAM viewer login',status==200 and json.loads(body).get('role')=='viewer',str(status))
    status,_,_=request(viewer,'/api/audit')
    check('SEC-07','Viewer audit forbidden',status==403,str(status))
    status,_,_=request(viewer,'/api/host/users')
    check('AUTH-06','Viewer inventory allowed',status==200,str(status))
    token=csrf(viewer)
    status,_,_=request(viewer,'/api/auth/logout',{},token)
    check('AUTH-07','Logout completes',status==204,str(status))
    status,_,_=request(viewer,'/api/auth/me')
    check('AUTH-08','Logged-out session denied',status==401,str(status))
    # Login budget includes prior calls. All subsequent attempts must become 429.
    statuses=[request(anon,'/api/auth/login',dict(userName=users[0],password='wrong'),csrf(anon))[0] for _ in range(6)]
    check('SEC-08','Login rate limiter reaches 429',429 in statuses,','.join(map(str,statuses)))
    status,body,_=request(anon,'/api/not-a-real-endpoint')
    check('API-05','Unknown API endpoint returns 404',status==404,str(status))
finally:
    for user in created:
        cmd(['userdel',user])
    check('UFW-02','UFW configuration unchanged',before==ufw_hash(),'Before/after /etc/ufw SHA256 equal' if before==ufw_hash() else 'Configuration changed')
    with open('/tmp/wallforge-validation-results.json','w') as f: json.dump(results,f,indent=2)
print('SUMMARY '+json.dumps({s:sum(x['status']==s for x in results) for s in ['PASS','FAIL']}))

if any(r["status"] != "PASS" for r in results): raise SystemExit(1)

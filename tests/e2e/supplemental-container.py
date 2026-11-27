import importlib.util, json, ssl, urllib.request, urllib.error, http.cookiejar, subprocess, secrets, struct
results=[]
def check(i,scenario,ok,actual):
    r=dict(id=i,scenario=scenario,status='PASS' if ok else 'FAIL',actual=actual);results.append(r);print(json.dumps(r),flush=True)
ctx=ssl.create_default_context(cafile='/opt/wallforge/tls/localhost.crt')
c=urllib.request.build_opener(urllib.request.HTTPSHandler(context=ctx),urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))
base='https://localhost:5544'
def req(path,data=None,headers=None):
    r=urllib.request.Request(base+path,data=data,headers=headers or {})
    try:
        with c.open(r,timeout=15) as resp:return resp.status,resp.read().decode()
    except urllib.error.HTTPError as e:return e.code,e.read().decode()
token=json.loads(req('/api/auth/csrf')[1])['token']
headers={'Content-Type':'application/json','X-WallForge-CSRF':token}
for i,name,pwd,expected in [('INPUT-01','bad;command','x',400),('INPUT-02','alex\n','x',400),('INPUT-03','a'*32+'$','x',400),('INPUT-04','unknown-user','x',401)]:
    status,body=req('/api/auth/login',json.dumps(dict(userName=name,password=pwd)).encode(),headers)
    check(i,'Reject invalid or unauthorized username',status==expected,str(status))
status,body=req('/api/auth/login',b'{broken',headers)
check('INPUT-05','Malformed JSON returns 400 without stack trace',status==400 and 'StackTrace' not in body,str(status))
for i,payload in enumerate([b'',b'\x00\x00\x00\x01x',b'\x00\x04\x00\x01rootx',b'\x00\x21\x00\x01'+b'a'*33+b'x',b'\x00\x04\x00\x01a\x00bcx']):
    r=subprocess.run(['runuser','-u','wallforge-api','--','sudo','-n','/usr/local/libexec/wallforge-pam-auth'],input=payload,capture_output=True)
    check(f'PAM-PROTO-{i+1}','Malformed helper protocol fails closed',r.returncode==0 and r.stdout==b'0','Denied' if r.stdout==b'0' else 'Unexpected response')
with open('/tmp/wallforge-supplemental-results.json','w') as f:json.dump(results,f,indent=2)

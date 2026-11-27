from pathlib import Path
import os,secrets,subprocess,json,struct
assert Path('/.dockerenv').exists() and os.geteuid()==0 and os.environ.get('WALLFORGE_DISPOSABLE_LAB')=='1', 'Explicit disposable Docker lab only'
user='wf-act-policy';password=secrets.token_hex(24);results=[]
def run(args,**kw):return subprocess.run(args,capture_output=True,timeout=20,**kw)
assert run(['id',user]).returncode!=0
def probe(name,expected):
 r=run(['runuser','-u','wallforge-api','--','/tmp/wallforge-validation-integration/WallForge.LinuxIntegration',user,'admin',expected],input=(password+'\n').encode())
 results.append(dict(scenario=name,status='PASS' if r.returncode==0 else 'FAIL',actual=r.stdout.decode().strip() or 'Harness rejected expectation'))
try:
 assert run(['useradd','-M','-s','/bin/bash','-G','wallforge-admin',user]).returncode==0
 assert run(['chpasswd'],input=(user+':'+password+'\n').encode()).returncode==0
 probe('Valid current local Ubuntu PAM account','valid')
 assert run(['usermod','-L',user]).returncode==0
 probe('Locked password denies PAM authentication','account-disabled')
 assert run(['usermod','-U',user]).returncode==0
 probe('Unlock restores valid authentication','valid')
 assert run(['chage','-d','0',user]).returncode==0
 probe('Password requiring change fails closed in noninteractive PAM helper','account-disabled')
 assert run(['chage','-d',__import__('datetime').date.today().isoformat(),user]).returncode==0
 assert run(['usermod','--expiredate','1',user]).returncode==0
 probe('Expired account denies PAM account-management phase','account-disabled')
 for name,payload in [('Root rejected',struct.pack('!HH',4,1)+b'rootx'),('Zero-length password rejected',struct.pack('!HH',len(user),0)+user.encode()),('System account rejected',struct.pack('!HH',len('wallforge-api'),1)+b'wallforge-apix')]:
  r=run(['runuser','-u','wallforge-api','--','sudo','-n','/usr/local/libexec/wallforge-pam-auth'],input=payload)
  results.append(dict(scenario=name,status='PASS' if r.returncode==0 and r.stdout==b'0' else 'FAIL',actual='Native helper denied' if r.stdout==b'0' else 'Unexpected response'))
finally:
 run(['userdel',user])
Path('/tmp/active-pam-policy-results.json').write_text(json.dumps(results,indent=2))
for result in results:print(result['status'],result['scenario'])
if any(r['status']!='PASS' for r in results):raise SystemExit(1)

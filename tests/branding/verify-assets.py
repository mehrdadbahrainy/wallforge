from pathlib import Path
import hashlib,json,sys
root=Path(__file__).resolve().parents[2]
assets=root/'frontend/public/brand'
expected=json.loads((assets/'approved-assets.sha256.json').read_text())
for name,digest in expected.items():
    assert hashlib.sha256((assets/name).read_bytes()).hexdigest()==digest,name
build=root/'frontend/dist/wallforge/browser'
for name,digest in expected.items():
    assert hashlib.sha256((build/'brand'/name).read_bytes()).hexdigest()==digest,name+' missing/changed in Production build'
assert hashlib.sha256((build/'favicon.ico').read_bytes()).hexdigest()==expected['favicon.ico']
html=(root/'frontend/src/app/app.component.html').read_text()
assert html.count('/brand/wallforge-horizontal.png')==2
assert 'brand-mark' not in html and 'class="shield"' not in html
assert 'favicon.ico' in (build/'index.html').read_text()
print('PASS approved asset hashes match source and Production output; header/login/favicon wired')

from pathlib import Path
import hashlib,json,sys,xml.etree.ElementTree as ET
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
vectors=json.loads((assets/'vector-assets.sha256.json').read_text())
for name,digest in vectors.items():
    assert hashlib.sha256((assets/name).read_bytes()).hexdigest()==digest,name
    assert hashlib.sha256((build/'brand'/name).read_bytes()).hexdigest()==digest,name+' changed in Production'
    svg=ET.parse(assets/name).getroot()
    assert svg.get('viewBox'),name+' missing scalable viewport'
    assert all(node.tag.split('}')[-1] in ('svg','path','title') for node in svg.iter()),name+' contains raster/active/background elements'
    assert not any(node.get('fill','').lower() in ('white','#fff','#ffffff') for node in svg.iter()),name+' contains a white backing'
assert html.count('/brand/wallforge-horizontal-light.svg')==2
assert 'brand-mark' not in html and 'class="shield"' not in html
assert 'favicon.ico' in (build/'index.html').read_text()
print('PASS original approved assets preserved; transparent path-only SVG derivatives match Production; header/login/favicon wired')

import '@angular/compiler';
import ts from 'typescript';
import { readFile, writeFile, mkdir, rm } from 'node:fs/promises';
import { pathToFileURL } from 'node:url';
import { resolve } from 'node:path';
import assert from 'node:assert/strict';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

const source = process.env.WF_COMPONENT_SOURCE || 'src/app/app.component.ts';
const cache = resolve('node_modules/.cache/wallforge-component-tests');
await mkdir(cache, { recursive: true });
const output = resolve(cache, 'component.mjs');
await writeFile(output, ts.transpileModule(await readFile(source, 'utf8'), {
  compilerOptions: { target: ts.ScriptTarget.ES2022, module: ts.ModuleKind.ES2022, experimentalDecorators: true }
}).outputText);
let passes = 0, failures = 0;
try {
  const { AppComponent } = await import(pathToFileURL(output).href);
  async function test(name, fn) {
    try { await fn(); passes++; console.log('PASS '+name); }
    catch(e) { failures++; console.log('FAIL '+name+': '+e.message); }
  }
  function fixture(status) {
    const http = {
      get: () => of({ token: 'test-only-token' }),
      post: () => status !== null ? throwError(() => new HttpErrorResponse({ status })) : of(null)
    };
    const app = new AppComponent(http);
    app.session.set({ userName: 'fixture', role: 'admin' });
    app.inventory.set({ users: [], groups: [] });
    app.firewall.set({ available: true, status: 'inactive', rules: [], error: null });
    app.events.set([{ when: 'test', event: 'test', actor: 'fixture' }]);
    app.currentView.set('activity');
    return app;
  }
  for (const status of [0, 400, 500]) {
    await test(`failed logout (${status}) preserves session and reports failure`, async () => {
      const app = fixture(status); await app.logout();
      assert.equal(app.session()?.userName, 'fixture');
      assert.match(app.error(), /could not be confirmed/);
    });
  }
  await test('successful logout clears all sensitive view state', async () => {
    const app=fixture(null); await app.logout();
    assert.equal(app.session(),null); assert.equal(app.inventory(),null);
    assert.equal(app.firewall(),null); assert.deepEqual(app.events(),[]);
    assert.equal(app.currentView(),'overview');
  });
  await test('expired logout clears local session', async () => {
    const app=fixture(401); await app.logout(); assert.equal(app.session(),null);
  });
  await test('401 refresh clears stale host data and activity', async () => {
    const app=fixture(null);
    app.http.get=()=>throwError(()=>new HttpErrorResponse({status:401}));
    await app.refresh();
    assert.equal(app.session(),null); assert.equal(app.inventory(),null);
    assert.equal(app.firewall(),null); assert.deepEqual(app.events(),[]);
    assert.match(app.error(),/expired/);
  });
  await test('login obtains a fresh token after session expiration', async () => {
    const app=fixture(null); app.session.set(null); app.csrf='';
    let posted=false;
    app.http.get=(path)=>of(path.endsWith('csrf') ? {token:'fresh-test-token'} : path.endsWith('audit') ? [] : {});
    app.http.post=()=>{ posted=true; return of({userName:'fixture',role:'admin'}); };
    await app.login(); assert.equal(posted,true); assert.equal(app.session()?.userName,'fixture');
  });
  console.log(`${passes} passed; ${failures} failed`);
  process.exitCode=failures?1:0;
} finally { await rm(output,{force:true}); }

import { Component, OnInit, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

interface Session { userName: string; role: 'admin' | 'viewer'; }
interface Firewall { available: boolean; status: string; rules: string[]; error: string | null; }
interface SystemUser { userName: string; uid: number; gid: number; home: string; shell: string; systemAccount: boolean; }
interface SystemGroup { name: string; gid: number; members: string[]; }
interface UserSnapshot { users: SystemUser[]; groups: SystemGroup[]; }
interface Audit { when: string; event: string; actor: string; }
type View = 'overview' | 'firewall' | 'users' | 'activity';

@Component({
  selector: 'wf-root',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  readonly session = signal<Session | null>(null);
  readonly currentView = signal<View>('overview');
  readonly firewall = signal<Firewall | null>(null);
  readonly inventory = signal<UserSnapshot | null>(null);
  readonly events = signal<Audit[]>([]);
  readonly loading = signal(true);
  readonly busy = signal(false);
  readonly error = signal('');
  readonly loginName = signal('');
  readonly loginPassword = signal('');
  private csrf = '';

  constructor(private readonly http: HttpClient) {}

  async ngOnInit(): Promise<void> {
    await this.ensureCsrf();
    await this.checkSession();
    this.loading.set(false);
  }
  private async ensureCsrf(): Promise<void> {
    try {
      const token = await firstValueFrom(this.http.get<{token: string}>('/api/auth/csrf', { withCredentials: true }));
      this.csrf = token.token;
    } catch {
      this.error.set('Cannot connect to WallForge API over a trusted HTTPS connection.');
    }
  }
  private async checkSession(): Promise<void> {
    try {
      const user = await firstValueFrom(this.http.get<Session>('/api/auth/me', { withCredentials: true }));
      this.session.set(user);
      await this.refresh();
    } catch {
      this.session.set(null);
    }
  }
  async login(): Promise<void> {
    if (this.busy() || !this.csrf) return;
    this.busy.set(true);
    this.error.set('');
    try {
      const user = await firstValueFrom(this.http.post<Session>('/api/auth/login',
        { userName: this.loginName(), password: this.loginPassword() },
        { headers: { 'X-WallForge-CSRF': this.csrf }, withCredentials: true }));
      this.session.set(user);
      this.loginPassword.set('');
      await this.refresh();
    } catch (e) {
      this.loginPassword.set('');
      this.error.set(e instanceof HttpErrorResponse && e.status === 429
        ? 'Too many login attempts. Try again later.'
        : 'Login was not accepted. Check your Ubuntu account and WallForge group.');
    } finally { this.busy.set(false); }
  }
  async logout(): Promise<void> {
    try {
      await firstValueFrom(this.http.post('/api/auth/logout', {},
        { headers: { 'X-WallForge-CSRF': this.csrf }, withCredentials: true }));
    } catch { /* session may have expired, still clear local state */ }
    this.session.set(null); this.inventory.set(null); this.firewall.set(null);
    this.events.set([]); this.currentView.set('overview');
    await this.ensureCsrf();
  }
  async refresh(): Promise<void> {
    if (!this.session()) return;
    this.busy.set(true);
    try {
      const [firewall, inventory] = await Promise.all([
        firstValueFrom(this.http.get<Firewall>('/api/host/firewall')),
        firstValueFrom(this.http.get<UserSnapshot>('/api/host/users'))
      ]);
      this.firewall.set(firewall);
      this.inventory.set(inventory);
      if (this.session()?.role === 'admin') {
        this.events.set(await firstValueFrom(this.http.get<Audit[]>('/api/audit')));
      } else this.events.set([]);
      this.error.set('');
    } catch (e) {
      if (e instanceof HttpErrorResponse && e.status === 401) {
        this.session.set(null); this.error.set('Your session has expired or access was revoked.');
      } else this.error.set('Some host information could not be loaded.');
    } finally { this.busy.set(false); }
  }
  go(view: View): void { this.currentView.set(view); }
}

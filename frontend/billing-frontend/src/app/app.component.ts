import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="app-container">
      <header class="gradient-header">
        <div class="container">
          <div class="flex align-items-center justify-content-between">
            <div>
              <h1 class="text-white m-0 mb-2 slide-in-left">
                <i class="pi pi-receipt mr-3"></i>
                Billing System
              </h1>
              <p class="text-white-alpha-90 m-0 text-sm fade-in">PostgreSQL Partitioned Tables Demo</p>
            </div>
            <div class="stat-card glass-effect">
              <i class="pi pi-database text-4xl mb-2" style="color: var(--primary-soft)"></i>
              <div class="text-sm font-semibold">Partition Based</div>
            </div>
          </div>
          <nav class="mt-4 flex gap-3">
            <a routerLink="/bills" routerLinkActive="active" class="nav-link">
              <i class="pi pi-list mr-2"></i>Bills
            </a>
            <a routerLink="/bills/create" routerLinkActive="active" class="nav-link">
              <i class="pi pi-plus mr-2"></i>Create Bill
            </a>
            <a routerLink="/reports" routerLinkActive="active" class="nav-link">
              <i class="pi pi-chart-bar mr-2"></i>Reports
            </a>
          </nav>
        </div>
      </header>
      
      <main class="app-main container py-5">
        <router-outlet></router-outlet>
      </main>
      
      <footer class="app-footer mt-5 p-4 text-center glass-effect">
        <small class="text-600">
          <i class="pi pi-code mr-2"></i>
          © 2024 Billing System | Built with Angular 18 + .NET 8 + PostgreSQL
        </small>
      </footer>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }
    
    .app-main {
      flex: 1;
    }
    
    .nav-link {
      text-decoration: none;
      padding: 0.75rem 1.5rem;
      border-radius: 8px;
      transition: all 0.3s ease;
      color: white;
      font-weight: 500;
      display: inline-flex;
      align-items: center;
      background: rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(10px);
    }
    
    .nav-link:hover {
      background: rgba(255, 255, 255, 0.2);
      transform: translateY(-2px);
    }
    
    .nav-link.active {
      background: rgba(255, 255, 255, 0.3);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }
    
    .app-footer {
      background: rgba(255, 255, 255, 0.8);
      border-top: 1px solid rgba(255, 255, 255, 0.3);
    }
  `]
})
export class AppComponent {
  title = 'billing-frontend';
}

import { Component, inject, OnInit, signal } from '@angular/core';
import { Header } from './header/header';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth-service';



@Component({
  imports: [Header, RouterOutlet],
  selector: 'app-root',
  standalone: true,
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private authService = inject(AuthService);

  protected readonly title = signal('moviesweb.client');

  ngOnInit(): void {
    this.authService.checkAuth().subscribe({
      error: (error: { status?: number }) => {
        if (error.status !== 401) {
          console.error('Could not restore the signed-in user:', error);
        }
      }
    });
  }
}

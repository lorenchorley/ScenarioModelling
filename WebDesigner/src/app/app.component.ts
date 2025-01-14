import { Component, Signal, signal, computed, WritableSignal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'WebDesigner';

  public ws : WritableSignal<number> = signal(0);
  public cs : Signal<number> = computed(() => this.ws() + 2);
}

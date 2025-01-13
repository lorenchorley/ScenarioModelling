import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { catchError, of } from 'rxjs'; 

@Component({
  selector: 'app-codeeditor',
  templateUrl: './codeeditor.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./codeeditor.component.css']
})
export class CodeeditorComponent implements OnInit {
  //public scenarioText: string = "";
  public readonly scenarioText$ = this.http.get<string>('http://localhost:7058/getscenario').pipe(
    catchError((error) => {
      return of(error);
    })
  );

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
  //  this.http.get<string>('/getscenario').subscribe(
  //    {
  //      next: (result) => {
  //        this.scenarioText = result;
  //      },
  //      error: (error) => {
  //        console.error(error);
  //      }
  //    }
  //  );
  }

}

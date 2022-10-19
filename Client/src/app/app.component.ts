import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { lastValueFrom, throwError } from 'rxjs';
import { catchError, map, retry, timeout } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Client';


  constructor(protected http: HttpClient){

  }

  ngOnInit(): void {
    const url = "http://localhost:30202/WeatherForecast";
    // this.http
    //     .post(url, "", {
    //       responseType: 'text',
    //       // headers: new HttpHeaders({
    //       //   'Access-Control-Allow-Origin': '*',
    //       // }),
    //     })
    //     .pipe(timeout(2500), retry(3), catchError(this.handleError))
    //     .pipe(map((response) => response))

    this.http.get(url)
      .pipe(timeout(2500), retry(3), catchError(this.handleError))
      .pipe(map((response) => console.log("response=>" + response)));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(`Backend returned code ${error.status}, body was: `, error.error);
    }
    // Return an observable with a user-facing error message.
    return throwError(() => 'Something bad happened; please try again later.');
  }
}

import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { lastValueFrom, throwError } from 'rxjs';
import { catchError, map, retry, timeout } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Client';

  private mHubConnection?: signalR.HubConnection;

  constructor(protected http: HttpClient) {

  }

  async ngOnInit(): Promise<void> {
    const url = "http://dev.hexawire.niji.ga:30202/Token/api/Token";
    console.log("ngOnInit");
    const accessToken = await lastValueFrom(
      this.http.post(url, null, { responseType: 'text' })
        .pipe(timeout(2500), retry(3), catchError(this.handleError))
        .pipe(map((response) => response))
  );

    console.log("アクセストークン: " + accessToken);

    this.mHubConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://dev.hexawire.niji.ga:30202" + '/front', {
        accessTokenFactory: () => accessToken,
      })
      .withAutomaticReconnect() // 自動再接続
      .build();

    try {
      await this.mHubConnection.start().catch(err => console.error(err));;
    } catch (error) {
      console.error('An error occurred:', error);
      return;
    }
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

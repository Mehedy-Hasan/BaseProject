import { AuthService } from './../../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {
  invalidLogin: boolean;

  constructor(
    private authServie: AuthService,
    private router: Router,
    private route: ActivatedRoute
    ) {}

  ngOnInit() {}

  signIn(credentials: any) {
    this.authServie.login(credentials).subscribe(
      result => {
        if (result) {
          let returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          this.router.navigate([returnUrl || '/']);
        } else {
          this.invalidLogin = true;
        }
      },
      error => {
        this.invalidLogin = true;
      }
    );
  }
}

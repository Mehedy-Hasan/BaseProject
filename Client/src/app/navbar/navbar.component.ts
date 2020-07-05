import { AuthService } from './../_services/auth.service';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  opened: boolean = true;

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  onClick() {
    this.opened = !this.opened;
    console.log(this.opened);
    this.authService.isOpen = this.opened;
  }

}

import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { User } from 'src/app/_models/User';
import { AuthService } from 'src/app/_services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
@ViewChild('inputFile', {static: true}) inputFile: ElementRef;
  user;
  registerForm: FormGroup;

  constructor(
    private authService: AuthService,
    private fb: FormBuilder,
    private route: Router) { }

  ngOnInit() {
    this.createRegisterFrom();
  }

  createRegisterFrom() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      userName: ['', Validators.required],
      fullName: ['', Validators.required],
      dateOfBirth: [{value: null, disabled: false}, Validators.required],
      address: ['', Validators.required],
      email: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      // image: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {validators: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'misMatch': true};
  }

  register() {
    this.user = Object.assign({}, this.registerForm.value);
    console.log(this.user);

    var nativeElement: HTMLInputElement = this.inputFile.nativeElement;
    var file = nativeElement.files[0];

    this.user.image = file;
    this.authService.register(this.user).subscribe(user => {
      console.log('success registration');
      this.registerForm.reset();
      // alertify.success('Registration Successfull');
    }, error => {
      console.log(error);
      // alertify.error('Registration Failed');
    },
    // () => {
    //   this.authService.login(this.user).subscribe(() => {
    //     this.route.navigate(['/login']);
    //   });
    //}
    );
  }
}

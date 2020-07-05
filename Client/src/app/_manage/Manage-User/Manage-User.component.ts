import { UserService } from './../../_services/user.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';

export interface UserData {
  id: number;
  userName: string;
  email: string;
  phoneNumber: string;
}

@Component({
  selector: 'app-Manage-User',
  templateUrl: './Manage-User.component.html',
  styleUrls: ['./Manage-User.component.css']
})
export class ManageUserComponent implements OnInit {

  displayedColumns: string[] = ['userName', 'email', 'phoneNumber', 'event'];
  dataSource: MatTableDataSource<UserData>;

  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  @ViewChild(MatSort, {static: true}) sort: MatSort;

  constructor(private userService: UserService) {  }

  ngOnInit() {
    this.userService.getUser().subscribe((x: UserData[]) => {
      this.dataSource = new MatTableDataSource(x);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  edit(id) {
console.log("do something", id);
  }
}

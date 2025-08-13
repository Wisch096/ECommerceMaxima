import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Department } from '../models/department';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class DepartmentsService {
  constructor(private api: ApiService) {}
  list(): Observable<Department[]> {
    return this.api.get<Department[]>('/departments');
    }
}
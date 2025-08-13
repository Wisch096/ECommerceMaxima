import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/paged-result';
import { Product, CreateProductRequest, UpdateProductRequest } from '../models/product';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  constructor(private api: ApiService) {}

  list(params: { search?: string; departmentCode?: string; isActive?: boolean; page?: number; pageSize?: number; }): Observable<PagedResult<Product>> {
    return this.api.get<PagedResult<Product>>('/products', params);
  }

  get(id: string): Observable<Product> {
    return this.api.get<Product>(`/products/${id}`);
  }

  create(body: CreateProductRequest): Observable<{ id: string }> {
    return this.api.post<{ id: string }>(`/products`, body);
  }

  update(id: string, body: UpdateProductRequest) {
    return this.api.put<void>(`/products/${id}`, body);
  }

  delete(id: string) {
    return this.api.delete<void>(`/products/${id}`);
  }
}

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Department } from 'src/app/core/models/department';
import { Product } from 'src/app/core/models/product';
import { DepartmentsService } from 'src/app/core/services/departments.service';
import { ProductsService } from 'src/app/core/services/products.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  loading = false;
  items: Product[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  search = '';
  departmentCode = '';
  isActive: '' | boolean = '';

  departments: Department[] = [];

  constructor(private products: ProductsService, private deps: DepartmentsService, private router: Router) {}

  ngOnInit(): void {
    this.loadDepartments();
    this.fetch();
  }

  loadDepartments() {
    this.deps.list().subscribe(d => this.departments = d);
  }

  fetch() {
    this.loading = true;
    const params: any = { page: this.page, pageSize: this.pageSize };
    if (this.search) params.search = this.search;
    if (this.departmentCode) params.departmentCode = this.departmentCode;
    if (this.isActive !== '') params.isActive = this.isActive;

    this.products.list(params).subscribe({
      next: res => { this.items = res.items; this.total = res.total; },
      error: () => {},
      complete: () => { this.loading = false; }
    });
  }

  resetFilters() {
    this.search = ''; this.departmentCode = ''; this.isActive = '';
    this.page = 1; this.fetch();
  }

  goNew() { this.router.navigate(['/products/new']); }
  goEdit(id: string) { this.router.navigate(['/products', id]); }

  confirmDelete(id: string) {
    if (!confirm('Confirma excluir (exclusão lógica)?')) return;
    this.products.delete(id).subscribe(() => this.fetch());
  }

  get totalPages() {
    return Math.max(1, Math.ceil(this.total / this.pageSize));
  }

  changePage(delta: number) {
    const next = this.page + delta;
    if (next < 1 || next > this.totalPages) return;
    this.page = next; this.fetch();
  }
}

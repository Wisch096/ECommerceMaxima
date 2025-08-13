import { Component, OnInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Department } from 'src/app/core/models/department';
import { UpdateProductRequest, CreateProductRequest } from 'src/app/core/models/product';
import { DepartmentsService } from 'src/app/core/services/departments.service';
import { ProductsService } from 'src/app/core/services/products.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  isEdit = false;
  id: string | null = null;
  departments: Department[] = [];

  form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(50)]], // somente no create
    description: ['', [Validators.required, Validators.maxLength(255)]],
    departmentCode: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    price: [0, [Validators.required, Validators.min(0)]],
    isActive: [true]
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private deps: DepartmentsService,
    private products: ProductsService
  ) {}

  ngOnInit(): void {
    this.deps.list().subscribe(d => this.departments = d);
    this.route.paramMap.subscribe(p => {
      const pid = p.get('id');
      if (pid) {
        this.isEdit = true; this.id = pid; this.form.get('code')?.disable();
        this.products.get(pid).subscribe(prod => {
          this.form.patchValue({
            code: prod.code,
            description: prod.description,
            departmentCode: prod.departmentCode,
            price: prod.price,
            isActive: prod.isActive
          });
        });
      }
    });
  }

  save() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    if (this.isEdit && this.id) {
      const body: UpdateProductRequest = {
        description: this.form.value.description!,
        departmentCode: this.form.value.departmentCode!,
        price: Number(this.form.value.price!),
        isActive: !!this.form.value.isActive
      };
      this.products.update(this.id, body).subscribe(() => this.router.navigate(['/products']));
    } else {
      const body: CreateProductRequest = {
        code: this.form.value.code!,
        description: this.form.value.description!,
        departmentCode: this.form.value.departmentCode!,
        price: Number(this.form.value.price!),
        isActive: !!this.form.value.isActive
      };
      this.products.create(body).subscribe(() => this.router.navigate(['/products']));
    }
  }

  cancel() { this.router.navigate(['/products']); }
}

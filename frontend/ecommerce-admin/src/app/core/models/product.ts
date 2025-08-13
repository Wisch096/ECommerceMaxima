export interface Product {
  id: string;
  code: string;
  description: string;
  departmentCode: string;
  price: number;
  isActive: boolean;
}

export interface CreateProductRequest {
  code: string;
  description: string;
  departmentCode: string;
  price: number;
  isActive: boolean;
}

export interface UpdateProductRequest {
  description: string;
  departmentCode: string;
  price: number;
  isActive: boolean;
}

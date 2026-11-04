export interface ApiValidationError {
  status?: number;
  message?: string;
  errors?: { [key: string]: string[] }; 
}
export interface UserLogin {
    Email: string,
    Password: string
}

export interface UserRegister {
    Login: string,
    Password: string,
    ConfirmPassword: string,
    UserName: string,
    Surname: string,
    Age: number,
    Email: string
}

export interface AuthResponse {
    token: string
}

export interface UserResponse {
    id: string,
    login: string,
    email: string,
    roles: string[],
}
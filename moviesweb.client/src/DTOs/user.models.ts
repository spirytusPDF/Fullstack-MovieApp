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
    Email: string,
    City?: string | null,
    FavouriteGenre?: string | null
}

export interface UserUpdate {
    Email: string,
    Password: string,
    NewPassword?: string | null,
    NewGenre?: string | null,
    NewLogin?: string | null,
    NewCity?: string | null
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

export interface UserProfile {
    id: string,
    login: string,
    userName: string,
    surname: string,
    city: string | null,
    favouriteGenre: string | null
}
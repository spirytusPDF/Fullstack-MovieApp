interface UserLogin{
    Login:string,
    Email:string,
    Password:string
}

interface UserRegister{
    Login:string,
    Password:string,
    ConfirmPassword:string,
    Name:string,
    Surname:string,
    Age:number,
    Email:string,
}

interface UserResponse{
    id:string,
    username:string,
    email:string,
}
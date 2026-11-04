interface CommentAdd{
    title:string,
    text:string,
}

interface CommentResponse{
    id:number,
    title:string,
    text:string,
    posted:string,
    movieName:string,
    userLogin:string
}
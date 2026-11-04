interface MovieAdd{
    title:string,
    description:string,
    genre:string,
    releaseYear:number,
    rating:number,
    posterUrl:string | null,
    movieUrl:string | null
}
interface MovieUpdate{
    description:string,
    genre:string,
    releaseYear:number,
    rating:number,
    posterUrl:string | null,
    movieUrl:string | null
}

interface MovieResponse{
     id:number,
    title:string,
    description:string,
    genre:string,
    releaseYear:number,
    rating:number,
    posterUrl:string | null,
    movieUrl:string | null,
    movieComments:CommentResponse[]
}
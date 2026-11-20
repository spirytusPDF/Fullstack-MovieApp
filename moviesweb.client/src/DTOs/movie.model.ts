import { MovieSortOrder } from "../app/services/movie-service";

export interface MovieAdd{
    title:string,
    description:string,
    genre:string,
    releaseYear:number,
    rating:number,
    duration:string,
    posterUrl:string | null,
    movieUrl:string | null
}
export interface MovieUpdate{
    description:string,
    genre:string,
    releaseYear:number,
    rating:number,
    duration:string,
    posterUrl:string | null,
    movieUrl:string | null
}

export interface MovieResponse{
     id:number,
    title:string,
    description:string,
    genre:string,
    releaseYear:number,
    rating:number,
    duration:string,
    posterUrl:string | null,
    movieUrl:string | null,
    movieComments:CommentResponse[]
}

export interface MovieCatalogQuery {
  filters: Partial<MovieFilterParams>;
  sort: MovieSortOrder;
  page: number;
  pageSize: number;
}

export interface MoviePagination {
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface MoviePage {
  movies: MovieResponse[];
  pagination: MoviePagination;
}
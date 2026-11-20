export interface SessionFilters {
  movieTitle?: string;
  hallName?: string;
  ticketPrice?: number;
  availableSeats?: number;
}

export interface MovieSessionResponse {
  id: number;
  startsAt: string;
  hallName: string;
  ticketPrice: number;
  availableSeats: number;
  movie: {
    title: string;
    rating: number;
    duration: string;
  };
}

export interface SessionPagination {
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface SessionPage {
  sessions: MovieSessionResponse[];
  pagination: SessionPagination;
}

export interface SessionCreate{
    startsAt: string;
    hallName: string;
    ticketPrice: number;
    availableSeats: number;
}

export interface SessionUpdate{
    startsAt: string;
    hallName: string;
    ticketPrice: number;
}
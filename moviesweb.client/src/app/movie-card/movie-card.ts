import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MovieResponse } from '../../DTOs/movie.model';

@Component({
  imports: [RouterLink, CommonModule],
  standalone:true,
  selector: 'app-movie-card',
  styleUrl: './movie-card.css',
  templateUrl: './movie-card.html',
})
export class MovieCard {
  @Input() movie!:MovieResponse;
  
}

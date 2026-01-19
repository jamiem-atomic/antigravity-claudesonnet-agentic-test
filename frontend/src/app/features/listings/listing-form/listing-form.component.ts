import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiService } from '../../../core/api.service';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-listing-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule],
  templateUrl: './listing-form.component.html',
  styleUrls: ['./listing-form.component.css']
})
export class ListingFormComponent implements OnInit {
  listingForm: FormGroup;
  isEditMode = false;
  listingId: string | null = null;
  isLoading = false;
  error = '';
  photoUrlInput = '';

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.listingForm = this.fb.group({
      title: ['', Validators.required],
      price: [null, [Validators.required, Validators.min(0)]],
      description: ['', Validators.required],
      make: ['', Validators.required],
      model: ['', Validators.required],
      year: [new Date().getFullYear(), [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      mileage: [null, [Validators.required, Validators.min(0)]],
      fuelType: ['Petrol', Validators.required],
      transmission: ['Automatic', Validators.required],
      bodyType: ['Sedan', Validators.required],
      condition: ['Used', Validators.required],
      location: ['', Validators.required],
      photos: [[]]
    });
  }

  ngOnInit() {
    // Check if edit mode
    this.listingId = this.route.snapshot.paramMap.get('id');
    if (this.listingId) {
      this.isEditMode = true;
      this.loadListing(this.listingId);
    }
  }

  loadListing(id: string) {
    this.isLoading = true;
    this.api.get<any>(`/listings/${id}`).subscribe({
      next: (data) => {
        this.listingForm.patchValue(data);
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load listing details.';
        this.isLoading = false;
      }
    });
  }

  addPhoto() {
    if (this.photoUrlInput) {
      const currentPhotos = this.listingForm.get('photos')?.value || [];
      this.listingForm.patchValue({
        photos: [...currentPhotos, this.photoUrlInput]
      });
      this.photoUrlInput = '';
    }
  }

  removePhoto(index: number) {
    const currentPhotos = this.listingForm.get('photos')?.value || [];
    currentPhotos.splice(index, 1);
    this.listingForm.patchValue({ photos: currentPhotos });
  }

  onSubmit() {
    if (this.listingForm.invalid) return;

    this.isLoading = true;
    this.error = '';

    const data = this.listingForm.value;

    const request$ = this.isEditMode
      ? this.api.put(`/listings/${this.listingId}`, data)
      : this.api.post('/listings', data);

    request$.subscribe({
      next: (res: any) => {
        const id = res.id || this.listingId;
        this.router.navigate(['/listings', id]);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to save listing.';
        this.isLoading = false;
      }
    });
  }
}

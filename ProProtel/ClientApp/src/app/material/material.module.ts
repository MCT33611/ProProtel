import { NgModule } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';



@NgModule({
  exports: [
    MatFormFieldModule,
    MatInputModule,
  ]
})
export class MaterialModule { }

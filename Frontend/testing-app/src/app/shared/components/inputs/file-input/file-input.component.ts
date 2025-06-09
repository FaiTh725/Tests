import { Component, Input, ElementRef, ViewChild, Output, EventEmitter } from '@angular/core';
import { FileStyle } from '../../../interfaces/inputs/FileStyle';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-file-input',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="file-input-main"
      (dragenter)="handleDragEnter($event)"
      (dragover)="handleDragEnter($event)"
      (dragleave)="handleDragLeave($event)"
      (drop)="handleDrop($event)"
      [ngClass]="{'file-input-focus': isInFocuse}"
      [style.--bg-color]="inputFileConf.BackgroundColor"
      [style.--bg-focus-color]="inputFileConf.BackgroundFocusColor"
      [style.--border-color]="inputFileConf.BorderColor"
      [style.--border-focus-color]="inputFileConf.BorderFocusColor"
      [style.--txt-color]="inputFileConf.TextColor"
      [style.--txt-focus-color]="inputFileConf.TextFocusColor"
      >
      <div class="file-input-wrapper">
        <div class="file-input-action-field"
          (click)="handleOpenFileDialog()">
          <p class="file-input-text">
            Drop here<br/>
              or<br/>
            click
          </p>
          <input 
            (change)="handleChanged($event)"
            class="file-input-input"
            type="file" 
            accept="image/png, image/jpeg" 
            multiple="true"
            #fileInput>
        </div>
      </div>
  `,
  styleUrl: './file-input.component.scss'
})
export class FileInputComponent {
  @Input() inputFileConf: FileStyle = {
    BackgroundColor: "var(--light-green-color)",
    BackgroundFocusColor: "var(--primary-write-color)",
    BorderColor: "var(--dark-green-color)",
    BorderFocusColor: "var(--primary-write-color)",
    TextColor: "var(--primary-write-color)",
    TextFocusColor: "var(--light-green-color)"
  }
  @Output() fileChange = new EventEmitter<File[]>();
  @ViewChild('fileInput', {static: false, read: ElementRef}) inputFile!: ElementRef;

  isInFocuse = false;
  isOpenUploadedPreview = false;

  get IsInFocuse() {
    return this.isInFocuse;
  }

  handleDragEnter(e: DragEvent) {
    e.preventDefault();

    this.isInFocuse = true;
  }

  handleDragLeave(e: DragEvent) {
    e.preventDefault();

    this.isInFocuse = false;
  }

  handleDrop(e: DragEvent) {
    e.preventDefault();

    if(e.dataTransfer?.files && e.dataTransfer?.files[0]) {
      const files = Array.from(e.dataTransfer?.files);
      
      this.fileChange.emit(files);
    }
  }

  handleChanged(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files[0]) {
      const  files = Array.from(input.files);

      this.fileChange.emit(files);
    }
  }

  handleOpenFileDialog() {
    this.inputFile.nativeElement.click();
  }
}

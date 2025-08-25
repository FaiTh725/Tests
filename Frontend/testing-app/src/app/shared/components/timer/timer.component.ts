import { DecimalPipe } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

@Component({
  selector: 'app-timer',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './timer.component.html',
  styleUrl: './timer.component.scss'
})
export class TimerComponent {
  private curTimeInSecond: number = 0;
  private maxTimeInSecond: number = 0;
  private intervalId: any;

  ctx: any;

  @Input() timerDurationInMinutes: number = 0;

  @ViewChild('canvas_progress', {static: true, read: ElementRef}) canvas!: ElementRef;

  get curMinutes(): number {
    return Math.floor(this.curTimeInSecond / 60);
  }

  get curSeconds(): number {
    return this.curTimeInSecond % 60;
  }

  public startTimer() {
    if(this.intervalId) {
      return;
    }

    this.curTimeInSecond = this.timerDurationInMinutes * 60;
    this.maxTimeInSecond = this.curTimeInSecond;

    this.intervalId = setInterval(() => {
      if(this.curTimeInSecond > 0) {
        this.curTimeInSecond --;
        this.drawProgress()
      }
      else {
        this.stopTimer();
      }
    }, 1000);
  }

  public stopTimer() {
    if(this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  ngOnDestriy() {
    this.stopTimer();
  }

  drawProgress() {
    const ctx = this.canvas.nativeElement.getContext('2d');
    const centerX = 50;
    const centerY = 50;
    const radius = 25;
    const startAngle = -0.5 * Math.PI;
    const endAngle = startAngle + (this.curTimeInSecond / this.maxTimeInSecond) * -2 * Math.PI;

    ctx.clearRect(0, 0, 100, 100);
    ctx.beginPath();
    ctx.moveTo(centerX, centerY);
    ctx.arc(centerX, centerY, radius, startAngle, endAngle);
    ctx.closePath();
    ctx.fillStyle = '#49c0f8';
    ctx.fill();
  }
}

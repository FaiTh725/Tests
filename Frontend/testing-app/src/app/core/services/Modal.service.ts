import { ComponentRef, EnvironmentInjector, Injectable, Type, ViewContainerRef } from '@angular/core';
import { Subject } from 'rxjs';
import { ModalComponent } from '../../shared/components/modal/modal.component';

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private modalRef!: ComponentRef<ModalComponent>;
  private innerRef!: ComponentRef<any>;
  private componentSubscriber!: Subject<string>;

  constructor(private injector: EnvironmentInjector) {
  }

  openModal<T>(entry: ViewContainerRef, innerComponent: Type<T>, data?: Partial<T>) {
    this.modalRef = entry.createComponent(ModalComponent, {
      environmentInjector: this.injector
    });

    const content = this.modalRef.instance.viewContainerRef;
    this.innerRef = content.createComponent(innerComponent, {
      environmentInjector: this.injector
    });

    this.modalRef.instance.close.subscribe(() => this.handleClose());
    this.modalRef.instance.confirm.subscribe(() => this.handleConfirm());
  
    if(data) {
      Object.assign(this.innerRef.instance, data);
    }

    this.componentSubscriber = new Subject<string>();
    return this.componentSubscriber.asObservable();
  }

  handleClose() {
    this.componentSubscriber.complete();
    this.innerRef.destroy();
    this.modalRef.destroy();
  }

  handleConfirm() {
    this.componentSubscriber.next("confirm");
    this.handleClose();
  }
}

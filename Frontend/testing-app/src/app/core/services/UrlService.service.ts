import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UrlService {

  public TransfromImagesDomain(images: string[]): string[] {
    return images.map(image => `url(${image.replace("azurite_storage", "localhost")})`);
  }

}

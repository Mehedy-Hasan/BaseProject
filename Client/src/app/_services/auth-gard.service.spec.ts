import { TestBed } from '@angular/core/testing';

import { AuthGard } from './auth-gard.service';

describe('AuthGardService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AuthGard = TestBed.get(AuthGard);
    expect(service).toBeTruthy();
  });
});

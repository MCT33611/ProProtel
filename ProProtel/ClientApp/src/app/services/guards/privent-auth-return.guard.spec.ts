import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { priventAuthReturnGuard } from './privent-auth-return.guard';

describe('priventAuthReturnGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => priventAuthReturnGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});

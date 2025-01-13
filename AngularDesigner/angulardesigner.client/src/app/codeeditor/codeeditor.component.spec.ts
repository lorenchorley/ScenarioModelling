import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CodeeditorComponent } from './codeeditor.component';

describe('CodeeditorComponent', () => {
  let component: CodeeditorComponent;
  let fixture: ComponentFixture<CodeeditorComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CodeeditorComponent]
    });
    fixture = TestBed.createComponent(CodeeditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

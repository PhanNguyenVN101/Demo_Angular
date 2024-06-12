import { ChangeDetectionStrategy, Component, OnInit } from "@angular/core";

@Component({
  selector: "app-mathang-management",
  templateUrl: "./mathang-management.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MatHangManagementComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}

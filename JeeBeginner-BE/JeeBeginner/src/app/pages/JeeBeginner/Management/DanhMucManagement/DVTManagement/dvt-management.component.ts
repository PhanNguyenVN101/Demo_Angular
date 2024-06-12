import { ChangeDetectionStrategy, Component, OnInit } from "@angular/core";

@Component({
  selector: "app-dvt-management",
  templateUrl: "./dvt-management.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DVTManagementComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}

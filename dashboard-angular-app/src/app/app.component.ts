import { Component, AfterViewInit, ElementRef, OnDestroy, Inject } from '@angular/core';
import { DashboardControl, ResourceManager, DashboardControlArgs } from 'devexpress-dashboard';
import { DOCUMENT } from "@angular/platform-browser";
import themes from "devextreme/ui/themes";

declare var require: (e) => any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit {
  private dashboardControl: DashboardControl;
  private colorSchemeIcon = '<svg id="colorSchemeIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><defs><style>.dx_gray{fill:#7b7b7b;}</style></defs><title>Themes copy</title><path class="dx_gray" d="M12,3a9,9,0,0,0,0,18c7,0,1.35-3.13,3-5,1.4-1.59,6,4,6-4A9,9,0,0,0,12,3ZM5,10a2,2,0,1,1,2,2A2,2,0,0,1,5,10Zm3,7a2,2,0,1,1,2-2A2,2,0,0,1,8,17Zm3-8a2,2,0,1,1,2-2A2,2,0,0,1,11,9Zm5,1a2,2,0,1,1,2-2A2,2,0,0,1,16,10Z" /></svg>';
  private isThemeReady: boolean = false;

  constructor(private element: ElementRef, @Inject(DOCUMENT) private document) {
  }

  ngAfterViewInit(): void {
    ResourceManager.registerIcon(this.colorSchemeIcon);
    this.switchThemes();
    themes.ready(() => {
      this.isThemeReady = true;
    });
  }
  switchThemes(): void {
    let theme = window.localStorage.getItem("dx-theme") || "light";
    if (theme === "light")
      return;

    this.document.getElementById('themeAnalytics').setAttribute('href', 'assets/css/analytics/dx-analytics.' + theme + '.css');
    this.document.getElementById('themeDashboard').setAttribute('href', 'assets/css/dashboard/dx-dashboard.' + theme + '.css');
    themes.current("generic." + theme);
  }

  onBeforeRender(args: DashboardControlArgs) {
    this.dashboardControl = args.component;
  }
  onDashboardTitleToolbarUpdated(args): void {
    var colorSchemaList = {
      "light": "Light",
      "dark": "Dark",
      "carmine": "Carmine",
      "greenmist": "Greenmist"
    };
    args.options.actionItems.unshift({
      type: "menu",
      icon: "colorSchemeIcon",
      hint: "Color Schema",
      menu: {
        items: Object.keys(colorSchemaList).map(function (key) { return colorSchemaList[key] }),
        type: 'list',
        selectionMode: 'single',
        selectedItems: [window.localStorage.getItem("dx-theme") || "light"],
        itemClick: function (data, element, index) {
          let newTheme = Object.keys(colorSchemaList)[index];
          window.localStorage.setItem("dx-theme", newTheme);
          window.location.reload();
        }
      }
    });
  }
}

<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/186794623/23.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T828717)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# HTML JS Dashboard - How to Switch Themes/Color Schemes

Our HTML JS Dashboard does not provide a way to switch themes in applications using the same mechanism as in our [online demo](https://demos.devexpress.com/Dashboard/) This example demonstrates a way to implement the required functionality in an Angular application.

## Files to Review

* [app.component.ts](./dashboard-angular-app/src/app/app.component.ts)
* [app.component.html](./dashboard-angular-app/src/app/app.component.html)

## Quick Start

In the **asp-net-core-server** folder run the following command:

```
dotnet run
```

In the **dashboard-angular-app** folder, run the following commands:

```
npm istall
npm start
```
## Example Overview

To implement switching themes functionality, follow the steps below:

1. Remove all style registrations related to the dashboard from the *styles.css* file.
1. Add theme CSS files from the node_modules folder to assets. See the [Angular.json](dashboard-angular-app/angular.json#L25-L40) file
1. To dynamically switch themes, include theme CSS files into the *index.html* page. Register CSS styles in the order shown in the [Apply a Default theme](https://docs.devexpress.com/Dashboard/119299#apply-a-built-in-theme) section:

    ```html
    <!doctype html>
    <html lang="en">
    <head>
      <meta charset="utf-8">
      <title>DashboardTest</title>
      <base href="/">
      <meta name="viewport" content="width=device-width, initial-scale=1">
      <link rel="icon" type="image/x-icon" href="favicon.ico">
      <link rel="stylesheet" type="text/css" href='/assets/css/devextreme/dx.common.css'/>
      <link rel="dx-theme" type="text/css" data-theme="generic.light" href='/assets/css/devextreme/dx.light.css' data-active="true" />
      <link rel="dx-theme" type="text/css" data-theme="generic.dark" href='/assets/css/devextreme/dx.dark.css' data-active="false" />
      <link rel="dx-theme" type="text/css" data-theme="genericatat.carmine" href='/assets/css/devextreme/dx.carmine.css' data-active="false" />
      <link rel="dx-theme" type="text/css" data-theme="genericatat.greenmist" href='/assets/css/devextreme/dx.greenmist.css' data-active="false" />
      <link rel="stylesheet" type="text/css" href='/assets/css/analytics/dx-analytics.common.css' />
      <link id="themeAnalytics" rel="stylesheet" type="text/css" href='/assets/css/analytics/dx-analytics.light.css' />
      <link rel="stylesheet" type="text/css" href='/assets/css/analytics/dx-querybuilder.css' />
      <link id="themeDashboard" rel="stylesheet" type="text/css" href='/assets/css/dashboard/dx-dashboard.light.css' />
    </head>
    <body>
      <app-root></app-root>
    </body>
    </html>
    ```

   Add DevExtreme themes as described in the following topic: [Switch Between Themes at Runtime](https://js.devexpress.com/Angular/Documentation/Guide/Themes_and_Styles/Predefined_Themes/#Switch_Between_Themes_at_Runtime).

4. Handle the `onDashboardTitleToolbarUpdated` event in the *app.component.ts* file and add a popup menu to the dashboard toolbar to switch themes:
 
    ```js  
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
      ngAfterViewInit(): void {  
        this.dashboardControl = new DashboardControl(this.element.nativeElement.querySelector(".dashboard-container"), {  
          // Specifies a URL of the Web Dashboard's server.  
          endpoint: "https://demos.devexpress.com/services/dashboard/api",  
          workingMode: "Designer",  
          extensions: {  
            'viewer-api': {  
              onDashboardTitleToolbarUpdated: this.onDashboardTitleToolbarUpdated  
            }  
          }  
        });  
    ```

    To register the color scheme icon, use the following code:

    ```js
    export classAppComponent implements AfterViewInit, OnDestroy {  
      private dashboardControl: DashboardControl;  
      private colorSchemeIcon = '<svg id="colorSchemeIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><defs><style>.dx_gray{fill:#7b7b7b;}</style></defs><title>Themes copy</title><path class="dx_gray" d="M12,3a9,9,0,0,0,0,18c7,0,1.35-3.13,3-5,1.4-1.59,6,4,6-4A9,9,0,0,0,12,3ZM5,10a2,2,0,1,1,2,2A2,2,0,0,1,5,10Zm3,7a2,2,0,1,1,2-2A2,2,0,0,1,8,17Zm3-8a2,2,0,1,1,2-2A2,2,0,0,1,11,9Zm5,1a2,2,0,1,1,2-2A2,2,0,0,1,16,10Z" /></svg>';  
      //..  
      ngAfterViewInit(): void {  
      //...  
      ResourceManager.registerIcon(this.colorSchemeIcon);  
      //...
    ```  
1. Import the themes property from the `devextreme/ui/themes` file to *app.component.ts*. This allows you to switch DevExtreme themes.
1. Import the Document property from the `@angular/platform-browser` file to *app.component.ts*. It is also necessary to inject the static document property. This will grant you access to the document property. You will be able to switch the Dashboard and Analytics themes.
1. Use the following code to switch themes and render `dashboardControl` after a DevExtreme theme is changed:

    ```js
    import { Component, AfterViewInit, ElementRef, OnDestroy, Inject } from '@angular/core';  
    import { DashboardControl, ResourceManager, DashboardPanelExtension } from 'devexpress-dashboard';  
    import { DOCUMENT } from "@angular/platform-browser";  
    import themes from "devextreme/ui/themes";  
    
    declare var require: (e) => any;  
     //...  
    export class DashboardComponent implements AfterViewInit, OnDestroy {  
      private dashboardControl: DashboardControl;  
      constructor(private element: ElementRef, @Inject(DOCUMENT) private document) {  
        ResourceManager.embedBundledResources();  
      }  
    //...  
      switchThemes() :void{  
        let theme = window.localStorage.getItem("dx-theme") || "light";  
        if (theme==="light")  
          return;  
         this.document.getElementById('themeAnalytics').setAttribute('href','assets/css/analytics/dx-analytics.'+theme+'.css');  
        this.document.getElementById('themeDashboard').setAttribute('href','assets/css/dashboard/dx-dashboard.'+theme+'.css');  
        themes.current("generic."+theme);  
      }  
      ngAfterViewInit(): void {  
        this.dashboardControl = new DashboardControl(this.element.nativeElement.querySelector(".dashboard-container"), {  
          // Specifies a URL of the Web Dashboard's server.  
          endpoint: "https://demos.devexpress.com/services/dashboard/api",  
          workingMode: "Designer",  
          extensions: {  
            'viewer-api': {  
              onDashboardTitleToolbarUpdated: this.onDashboardTitleToolbarUpdated  
            }  
          }  
        });  
        ResourceManager.registerIcon(this.colorSchemeIcon);  
        this.switchThemes();  
        let db = this.dashboardControl;  
        themes.ready(function () {  
          db.render();  
        });  
      }  
    //...  
    } 
    ```

1. If you face any issues with the Dashboard menu (for example, you can't close it after a theme is switched), it is necessary to use the following CSS:

    ```css
    .dx-dashboard-dashboard-form.dx-overlay-wrapper {  
      left: 240px;  
    }   
    ```
## Documentation

* [Themes and Styles for an Angular Application](https://docs.devexpress.com/Dashboard/402098)

## More Examples

[Dashboard for Angular - Get Started](https://github.com/DevExpress-Examples/dashboard-angular-app-get-started)

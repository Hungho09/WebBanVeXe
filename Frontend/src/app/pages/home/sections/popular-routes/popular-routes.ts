import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

/**
 * PopularRoutesComponent — Hiển thị tuyến đường phổ biến
 * Input: routes — danh sách từ CmsConfig.popularRoutes
 */
@Component({
    selector: 'app-popular-routes',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './popular-routes.html',
    styleUrl: './popular-routes.css',
})
export class PopularRoutesComponent {
    @Input() routes: any[] = [];
    readonly defaultRouteImg = 'https://img.freepik.com/free-photo/beautiful-view-ha-long-bay-vietnam_181624-34539.jpg?w=740';

    constructor(private router: Router) {}

    formatRouteName(name: string): string {
        return name.includes('->') ? name.replace('->', ' ➔ ') : name;
    }

    onBookRoute(pr: any) {
        if (pr.routeId) {
            this.router.navigate(['/search-results'], { 
                queryParams: { 
                    routeId: pr.routeId,
                    routeName: pr.routeName
                } 
            });
        }
    }
}

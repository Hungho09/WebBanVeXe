import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

interface Resource {
  id: number;
  name: string;
  type: string;
  status: 'Available' | 'Maintenance' | 'Out of Service';
  description: string;
}

@Component({
  selector: 'app-resource-management',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './resource-management.html',
  styleUrls: ['./resource-management.css']
})
export class ResourceManagement implements OnInit {
  resourceList: Resource[] = [
    { id: 1, name: 'Bus 001 - Mercedes-Benz', type: 'Bus', status: 'Available', description: 'VIP Sleeper bus with 34 cabins.' },
    { id: 2, name: 'Bus 002 - Thaco Mobihome', type: 'Bus', status: 'Maintenance', description: 'Standard sleeper bus, 40 seats.' },
    { id: 3, name: 'Hanoi Central Station', type: 'Station', status: 'Available', description: 'Main pickup point in Hanoi.' },
    { id: 4, name: 'Ho Chi Minh Terminal', type: 'Station', status: 'Available', description: 'South hub for all routes.' },
    { id: 5, name: 'Bus 003 - Ford Transit', type: 'Van', status: 'Out of Service', description: 'Mini bus for short distance.' },
  ];

  filteredList: Resource[] = [];
  searchTerm: string = '';
  
  isModalOpen = false;
  resourceForm: FormGroup;
  isEditMode = false;
  currentResourceId: number | null = null;

  constructor(private fb: FormBuilder) {
    this.resourceForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      type: ['', Validators.required],
      status: ['Available', Validators.required],
      description: ['']
    });
  }

  ngOnInit() {
    this.filteredList = [...this.resourceList];
  }

  onSearch() {
    this.filteredList = this.resourceList.filter(item => 
      item.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      item.type.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  openAddModal() {
    this.isEditMode = false;
    this.currentResourceId = null;
    this.resourceForm.reset({ status: 'Available' });
    this.isModalOpen = true;
  }

  openEditModal(resource: Resource) {
    this.isEditMode = true;
    this.currentResourceId = resource.id;
    this.resourceForm.patchValue({
      name: resource.name,
      type: resource.type,
      status: resource.status,
      description: resource.description
    });
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  saveResource() {
    if (this.resourceForm.valid) {
      const formValue = this.resourceForm.value;
      
      if (this.isEditMode && this.currentResourceId !== null) {
        // Update
        const index = this.resourceList.findIndex(r => r.id === this.currentResourceId);
        if (index !== -1) {
          this.resourceList[index] = { ...formValue, id: this.currentResourceId };
        }
      } else {
        // Add
        const newId = this.resourceList.length > 0 ? Math.max(...this.resourceList.map(r => r.id)) + 1 : 1;
        this.resourceList.push({ ...formValue, id: newId });
      }
      
      this.onSearch();
      this.closeModal();
    }
  }

  onDelete(id: number) {
    if (confirm('Bạn có chắc chắn muốn xóa tài nguyên này?')) {
      this.resourceList = this.resourceList.filter(item => item.id !== id);
      this.onSearch();
    }
  }

  getStatusClass(status: string) {
    switch (status) {
      case 'Available': return 'status-available';
      case 'Maintenance': return 'status-maintenance';
      default: return 'status-out';
    }
  }
}
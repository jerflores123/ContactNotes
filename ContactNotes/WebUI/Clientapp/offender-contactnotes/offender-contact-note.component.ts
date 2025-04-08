import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { UiCommonService } from '../ui-common/ui-common-service';
import { GetOffenderContactNoteBySinResponse, OffenderContactNotesClient, PrivilegeClient, GetCurrentUserFeaturePrivilegeResponse, } from '../web-api-client';
import { OffenderContactNotesDetailsComponent } from './offender-contactnotes-details/offender-contactnote-details.component';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

declare const bootbox: any;

@Component({
  selector: 'app-offender-contact-note',
  templateUrl: './offender-contact-note.component.html',
  styleUrls: ['./offender-contact-note.component.scss']
})
export class OffenderContactNoteComponent implements OnInit {
  staffKey: number;
  _sin: string;
  newListEditor: any = {};
  vm: GetOffenderContactNoteBySinResponse[];
  Privilege: GetCurrentUserFeaturePrivilegeResponse;
  public isAgencySame: boolean = sessionStorage.getItem('isAgencySame') == "true";

  constructor(private privilegeClient: PrivilegeClient, private uiService: UiCommonService, private route: ActivatedRoute, private offenderContactNoteClient: OffenderContactNotesClient, public dialog: MatDialog, authorizeService: AuthorizeService) {
    this._sin = this.route.snapshot.paramMap.get('id');
    this.staffKey = authorizeService.currentUserValue.staffKey;
    this.privilegeClient.getCurrentUserFeaturePrivilege('Contact Notes').subscribe(
      result => {
        this.Privilege = result;
        if (this.Privilege.read_priv)
          this.LoadGrid(this._sin);
      },
      error => {
        console.error(error)
      }
    );
  }

  DeleteItem(Contact_number): void {
    bootbox.confirm({
      message: "<i class='fa-solid fa fa-exclamation-triangle fa-3x float-left pr-3' style='color:red;'></i> Are you sure you want to remove selected Contact Note? ",
      centerVertical: true,
      buttons: {
        confirm: {
          label: 'Yes',
          className: 'btn-primary'
        },
        cancel: {
          label: 'Cancel',
          className: 'btn-close mr-3'
        }
      },
      callback: (result) => {
        if (result) {
          this.offenderContactNoteClient.delete(Contact_number)
            .subscribe(
              _ => {
                this.uiService.snackNotification('Contact Note removed successfully!');
                this.LoadGrid(this._sin);
              },
              error => { console.dir(error.response); });
        }
      }
    });
  }

  ngOnInit(): void { }


  public LoadGrid(id) {
    this.offenderContactNoteClient.getAll(id).subscribe(
      result => {
        this.vm = result;
      },
      error => console.error(error)
    );
  }

  OpenItem(Contact_number, cansaveedit, IsAdd) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.id = "contact-method-modal-component";
    dialogConfig.height = 'fit-content';
    dialogConfig.width = "700px";
    dialogConfig.data = { sin: this._sin, Contact_number: Contact_number, Mythis: this, cansaveedit: cansaveedit, isAdd: IsAdd };
    this.dialog.open(OffenderContactNotesDetailsComponent, dialogConfig);
  }
}

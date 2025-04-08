import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UiCommonService } from '../../ui-common/ui-common-service';
import { OffenderContactNoteVM, OffenderContactNotesClient, CreateOffenderContactNoteCommand, UpdateOffenderContactNoteCommand, ContactTypeDto, LookupsClient } from '../../web-api-client';
import { DatePipe } from '@angular/common';

export interface DialogData {
  Contact_number: number;
  sin: number;
  Mythis: any;
  cansaveedit: boolean;
  isAdd: boolean;
}

@Component({
  selector: 'app-offender-contactnote-details',
  templateUrl: './offender-contactnote-details.component.html',
  styleUrls: ['./offender-contactnote-details.component.scss']
})
export class OffenderContactNotesDetailsComponent implements OnInit {
  vm: OffenderContactNoteVM;
  contactTypes: ContactTypeDto[];
  newListEditor: any = {};
  disableButton = false;
  ContactNote: boolean = false;
  datepipe: DatePipe = new DatePipe('en-US')
  public todayDate: any = new Date();
  today = new Date();
  maxDate = this.today?.toISOString()?.slice(0, 10);

  constructor(
    private offenderContactnoteClient: OffenderContactNotesClient,
    private lookupsClient: LookupsClient,
    private uiService: UiCommonService,
    public dialogRef: MatDialogRef<OffenderContactNotesDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {
    this.LoadGrid(data.sin, data.Contact_number, data.cansaveedit);
  }

  ngOnInit(): void { }

  LoadGrid(sin, id, isAdd) {
    let agencyid = Number(sessionStorage.getItem('agency_id'));
    this.lookupsClient.getContactTypes().subscribe(result => this.contactTypes = result);
    this.offenderContactnoteClient.get(sin, id, agencyid).subscribe(
      result => this.vm = result,
      error => console.error(error)
    );
  }

  UpdateItem(): void {
    const command = new UpdateOffenderContactNoteCommand({
      contactedById: this.vm.juvenile_ContactNoteDto.contacted_by,
      comment: this.vm.juvenile_ContactNoteDto.comment,
      contactDate: this.vm.juvenile_ContactNoteDto.contact_date,
      contactTypeId: this.vm.juvenile_ContactNoteDto.contact_type_id,
      contactNoteId: this.vm.juvenile_ContactNoteDto.contact_number,
      isFamilyOrGuardianInvolved: this.vm.juvenile_ContactNoteDto.isFamilyOrGuardianInvolved,
      isJuvenileInvolved: this.vm.juvenile_ContactNoteDto.isJuvenileInvolved
    });
    this.disableButton = true;
    this.newListEditor = [];
    this.offenderContactnoteClient.update(command)
      .subscribe(
        _ => {
          this.disableButton = false;
          this.uiService.snackNotification('Contact Note updated successfully!');
          this.dialogRef.close();
          this.data.Mythis.LoadGrid(this.data.sin);
        },
        error => {
          this.disableButton = false;
          let errors = JSON.parse(error.response);
          if (errors) {
            if (errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contact_type"]) {
              this.newListEditor.contact_type = errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contact_type"][1];
            }
            if (errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contacted_by"]) {
              this.newListEditor.contacted_by = errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contacted_by"][0];
            }
            if (errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Comment"]) {
              this.newListEditor.comment = errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Comment"][0];
            }
          }
          setTimeout(() => document.getElementById("title").focus(), 250);
        }
      );
  }

  createItem(): void {
    const command = new CreateOffenderContactNoteCommand({
      contactedById: this.vm.juvenile_ContactNoteDto.contacted_by,
      comment: this.vm.juvenile_ContactNoteDto.comment,
      contactDate: this.vm.juvenile_ContactNoteDto.contact_date,
      contactTypeId: this.vm.juvenile_ContactNoteDto.contact_type_id,
      sin: this.data.sin,
      isFamilyOrGuardianInvolved: this.vm.juvenile_ContactNoteDto.isFamilyOrGuardianInvolved,
      isJuvenileInvolved: this.vm.juvenile_ContactNoteDto.isJuvenileInvolved
    });
    this.disableButton = true;
    this.newListEditor = [];
    this.offenderContactnoteClient.create(command)
      .subscribe(
        _ => {
          this.disableButton = false;
          this.uiService.snackNotification('Contact Note added successfully!');
          this.dialogRef.close();
          this.data.Mythis.LoadGrid(this.data.sin);
        },
        error => {
          this.disableButton = false;
          let errors = JSON.parse(error.response);
          if (errors) {
            if (errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contact_type"]) {
              this.newListEditor.contact_type = errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contact_type"][1];
            }
            if (errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contacted_by"]) {
              this.newListEditor.contacted_by = errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Contacted_by"][0];
            }
            if (errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Comment"]) {
              this.newListEditor.comment = errors.errors["OffenderContactNoteVM.Juvenile_ContactNoteDto.Comment"][1];
            }
          }
          setTimeout(() => document.getElementById("title").focus(), 250);
        });
  }
}
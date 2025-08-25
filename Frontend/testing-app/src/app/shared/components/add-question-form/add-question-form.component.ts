import { Component, EventEmitter, Output } from '@angular/core';
import { PrimaryInputComponent } from "../inputs/primary-input/primary-input.component";
import { CheckboxComponent } from "../inputs/checkbox/checkbox.component";
import { PrimaryButtonComponent } from "../buttons/primary-button/primary-button.component";
import { SquareCheckboxComponent } from "../inputs/square-checkbox/square-checkbox.component";
import { FileInputComponent } from "../inputs/file-input/file-input.component";

@Component({
  selector: 'app-add-question-form',
  standalone: true,
  imports: [PrimaryInputComponent, PrimaryButtonComponent, CheckboxComponent, SquareCheckboxComponent, FileInputComponent],
  templateUrl: './add-question-form.component.html',
  styleUrl: './add-question-form.component.scss'
})
export class AddQuestionFormComponent {
  @Output() addQuestion = new EventEmitter<AddQuestionForm>();
  addQuestionForm: AddQuestionForm = {
    Answers: [],
    QuestionText: "",
    QuestionWeight: "",
    HasOneAnswer: true,
    QuestionImages: []
  }

  addAnswerForm: AddAnswerForm = {
    Answer: "",
    IsCorrect: false,
    AnswerImages: []
  }

  addQuestionErrorForm: AddQuestionErrorForm = {
    QuestionTextError: "",
    QuestionWeightError: "",
    CommonErrorError: ""
  }

  addAnswerErrorForm: AddAnswerErrorForm = {
    AnswerError: ""
  }
  
  handleAddAnswer() {
    if(this.addQuestionForm.HasOneAnswer &&
      this.addAnswerForm.IsCorrect &&
      this.addQuestionForm.Answers.filter(answer => answer.IsCorrect).length == 1
    ) {
      this.addQuestionErrorForm.CommonErrorError = "Question already has right answer";    
      
      return;
    }

    this.addQuestionForm.Answers.push({
      Answer: this.addAnswerForm.Answer,
      IsCorrect: this.addAnswerForm.IsCorrect,
      AnswerImages: []
    });

    this.addAnswerForm = {
      Answer: "",
      IsCorrect: false,
      AnswerImages: []
    }
  }

  handleAddQuestion() {
    var isValidForm = true;

    if(this.addQuestionForm.QuestionText.length < 2 ||
      this.addQuestionForm.QuestionText.length > 300
    ) {
      this.addQuestionErrorForm.QuestionTextError = "Question must be in range from 2 to 300"
      isValidForm = false;
    }
    const questionWeight = Number(this.addQuestionForm.QuestionWeight)
    if(isNaN(questionWeight) ||
      questionWeight < 0) {
        this.addQuestionErrorForm.QuestionWeightError = "Weight must be number and greater than zero"
      isValidForm = false;
    }
    if(this.addQuestionForm.Answers.length == 0) {
      this.addQuestionErrorForm.CommonErrorError = "Question must have 1 correct answer at least"
      isValidForm = false;
    }
    else if(this.addQuestionForm.HasOneAnswer &&
      this.addQuestionForm.Answers.filter(answer => answer.IsCorrect).length > 1
    )
    {
      this.addQuestionErrorForm.CommonErrorError = "Question has more than 1 answer"
      isValidForm = false;
    }

    if(!isValidForm) {
      return;
    }

    this.addQuestion.emit(this.addQuestionForm);

    this.addQuestionForm = {
      Answers: [],
      QuestionText: "",
      QuestionWeight: "",
      HasOneAnswer: true,
      QuestionImages: []
    }
  }

  clearQuestionErrorForm() {
    this.addQuestionErrorForm = {
      QuestionTextError: "",
      QuestionWeightError: "",
      CommonErrorError: ""
    }
  }

  clearAnswerErrorForm() {
    this.addAnswerErrorForm = {
      AnswerError: ""
    }
  }
}

interface AddAnswerForm {
  Answer: string,
  IsCorrect: boolean,
  AnswerImages: File[]
}

export interface AddQuestionForm {
  QuestionText: string,
  QuestionWeight: string,
  HasOneAnswer: boolean,
  Answers: AddAnswerForm[],
  QuestionImages: File[]
}

interface AddQuestionErrorForm {
  QuestionTextError: string,
  QuestionWeightError: string,
  CommonErrorError: string
}

interface AddAnswerErrorForm {
  AnswerError: string
}

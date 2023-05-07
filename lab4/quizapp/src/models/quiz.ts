import { Question } from "./question";
import { QuizMetaData } from "./quizMetaData";

export type Quiz = QuizMetaData & {
  questions: Question[];
};

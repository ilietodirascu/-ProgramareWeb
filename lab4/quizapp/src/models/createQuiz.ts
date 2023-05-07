type Question = {
  question: string;
  answers: string[];
  correct_answer: string;
};
export type CreateQuiz = {
  title: string;
  questions: Question[];
};

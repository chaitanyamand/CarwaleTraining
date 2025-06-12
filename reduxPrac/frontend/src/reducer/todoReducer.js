import { nanoid } from "nanoid";
import { FETCH_TODOS_SUCCESS, FETCH_TODOS_FAILED, FETCH_TODOS_PENDING,ADD_TODO,REMOVE_TODO } from "../actions/actions";

const initialState = {
  data: [],
  status : "pending"
};

export const todoReducer = (state = initialState, action) => {
  switch (action.type) {
    case FETCH_TODOS_PENDING:
      return { ...state, status: "pending"};
    case FETCH_TODOS_SUCCESS:
      return { ...state, status : "success", data : action.payload };
    case FETCH_TODOS_FAILED:
      return { ...state, status : "failed"};
    case ADD_TODO: 
        const todo = {id: nanoid(), text:action.payload}
        state.data.push(todo);
        return state;
    case REMOVE_TODO : 
        return {...state,data : state.data.filter((todo)=> todo.id !== action.payload)} 
        
    default:
      return state;
  }
};

import axios from "axios";
import { FETCH_TODOS_FAILED, FETCH_TODOS_PENDING, FETCH_TODOS_SUCCESS } from "../actions/actions";

export const fetchTodos =  () => {
    return async (dispatch) => {
            dispatch({type : FETCH_TODOS_PENDING})

            try {
                const todos = await axios.get("http://localhost:8080/todos");
                dispatch({type : FETCH_TODOS_SUCCESS, payload : todos.data});
            }

            catch{
                dispatch({type : FETCH_TODOS_FAILED})
            }
    }
}
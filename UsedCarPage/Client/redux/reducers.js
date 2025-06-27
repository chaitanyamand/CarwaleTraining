import { combineReducers } from "redux";
import { filterReducer } from "./filterReducer";
import { productReducer } from "./productReducer";

export const rootReducer = combineReducers({ filter: filterReducer, product: productReducer },)
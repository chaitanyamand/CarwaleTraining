import { Component } from 'react'
import './App.css'
import { connect } from 'react-redux'
import { fetchTodos } from './thunk/thunk';
import { ADD_TODO, REMOVE_TODO } from './actions/actions';



class App extends Component {
  constructor(props){
    super(props);
    this.state = {
      input : ""
    }
  }

  componentDidMount(){
    this.props.fetchTodos();
  }

  handleAdd = () => {
     this.props.dispatch({type : ADD_TODO, payload : this.state.input
     });
     this.setState({input : ""})
  }

  handleDelete = (id) => {
    this.props.dispatch({type : REMOVE_TODO, payload : id})
  }

  render() {
    const {status, data} = this.props;
    if(status == "pending")
    {
      return <h1>Loading Todos</h1>
    }
    else if(status == "failed")
    {
      return <h1>Failed to load Todos</h1>
    }
    return (
      <>
      <div>
        <input style={{margin : "20px"}} type='text' value={this.state.input} onChange={(event) => {this.setState({input : event.target.value})}} name="input"></input>
        <button onClick={this.handleAdd}>Add</button>
      </div>
      <div>
        {data && data.map && data.map(todo => {
          return (<div style={{display:"flex", justifyContent:"space-between", margin:"20px"}} key={todo.id}>
            <h3>
              {todo.text}
            </h3>
            <button onClick={() => this.handleDelete(todo.id)}>
              Delete
            </button>
          </div>)
        })}
      </div>
      </>
    )
  }

}

const mapStateToProps = (state) => ({
  status : state.todos.status,
  data : state.todos.data
});


const mapDispatchToProps = (dispatch) => ({
  fetchTodos: () => dispatch(fetchTodos()),
  dispatch,
});


export default connect(mapStateToProps, mapDispatchToProps)(App);


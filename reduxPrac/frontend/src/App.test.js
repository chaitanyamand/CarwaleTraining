import React from 'react';
import { shallow } from 'enzyme';
import App from './App';

describe('App', () => {
  it('renders heading', () => {
    const wrapper = shallow(<App />);
    expect(wrapper.text()).toContain('Todo');
  });
});

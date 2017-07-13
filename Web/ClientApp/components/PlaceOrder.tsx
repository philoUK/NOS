import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { OrderApi } from '../api/OrderApi';

function CustomTextInput(props) {
    return <input type="text" placeholder={props.placeholder} ref={props.inputRef} />;
}

export default class PlaceOrder extends React.Component<RouteComponentProps<{}>, {}> {

    private reference: HTMLInputElement;
    private customer: HTMLInputElement;

    constructor(props) {
        super(props);
    }

    placeOrder() {
        OrderApi.placeOrder(this.reference.value, this.customer.value);
        this.reference.value = '';
        this.customer.value = '';
    }

    public render() {
        return <div>
                   <header>Order Form
                   </header>
                   <p>
                    <CustomTextInput placeholder="Reference Code" inputRef={el => this.reference = el} />
                   </p>
                   <p>
                    <CustomTextInput placeholder="Customer Code" inputRef={el => this.customer = el} />
                   </p>
                   <button onClick={this.placeOrder.bind(this)}>Submit</button>
               </div>;
    }
}

//type PlaceOrderProps = typeof actionCreators & RouteComponentProps<{}>;

//export default class PlaceOrder extends React.Component<PlaceOrderProps, {}> {
//    public render() {
//        return <div>
//                <fieldset>
//                    <legend>Place Order</legend>
//                </fieldset>
//                <p>
//                   <input type="text" ref="refcode" placeholder="unique reference code" />
//                </p>
//                <p>
//                   <input type="text" ref="cust" placeholder="customer code" />
//                </p>
//               </div>;
//    }
//}

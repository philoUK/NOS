import { fetch } from 'domain-task';

const api = {
    placeOrder(reference: string, customer: string): void {
        var payload = { customer: customer, reference: reference };
        fetch('/api/order',
                {
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    method: 'post',
                    body: JSON.stringify(payload)
                })
            .then(res => console.log(res))
            .catch(res => console.log(res));
    }
}

export { api as OrderApi };